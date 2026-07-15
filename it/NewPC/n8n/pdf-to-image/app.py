from fastapi import FastAPI, Request, HTTPException
from pdf2image import convert_from_bytes, pdfinfo_from_bytes
from PIL import Image, ImageDraw, ImageFont
import base64
import io
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(title="PDF to Image Converter")

DPI = 200
MAX_DIM = 2000
MIN_DPI = 40
MAX_DPI = 400
MAX_SELECTED_PAGES = 20
BANNER_HEIGHT = 48


def _banner_font():
    try:
        return ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 32)
    except Exception:
        try:
            return ImageFont.load_default(size=32)
        except TypeError:
            return ImageFont.load_default()


def _stamp(img, page_num, total):
    # canvas is expanded above the page so the banner never covers content
    stamped = Image.new("RGB", (img.width, img.height + BANNER_HEIGHT), "black")
    stamped.paste(img, (0, BANNER_HEIGHT))
    draw = ImageDraw.Draw(stamped)
    text = f"IMG {page_num} OF {total}" if total else f"IMG {page_num}"
    draw.text((12, 8), text, fill="white", font=_banner_font())
    return stamped


@app.post("/pageinfo")
async def page_info(request: Request):
    pdf_bytes = await request.body()
    if not pdf_bytes:
        raise HTTPException(status_code=400, detail="No PDF data received")

    try:
        info = pdfinfo_from_bytes(pdf_bytes)
    except Exception as e:
        logger.error(f"PDF info read failed: {e}")
        raise HTTPException(status_code=422, detail=f"PDF info read failed: {str(e)}")

    return {"pages": int(info.get("Pages", 0))}


@app.post("/convert")
async def convert_pdf(request: Request):
    pdf_bytes = await request.body()
    if not pdf_bytes:
        raise HTTPException(status_code=400, detail="No PDF data received")

    try:
        dpi = int(request.query_params.get("dpi", DPI))
    except ValueError:
        raise HTTPException(status_code=400, detail="dpi must be an integer")
    if not MIN_DPI <= dpi <= MAX_DPI:
        raise HTTPException(status_code=400, detail=f"dpi must be between {MIN_DPI} and {MAX_DPI}")

    pages_param = request.query_params.get("pages")
    page_numbers = None
    if pages_param:
        try:
            page_numbers = sorted({int(p) for p in pages_param.split(",") if p.strip()})
        except ValueError:
            raise HTTPException(status_code=400, detail="pages must be comma-separated integers")
        if not page_numbers or page_numbers[0] < 1 or len(page_numbers) > MAX_SELECTED_PAGES:
            raise HTTPException(
                status_code=400,
                detail=f"pages must list 1-{MAX_SELECTED_PAGES} positive page numbers",
            )

    label = request.query_params.get("label")
    doc_total = None
    if label:
        try:
            doc_total = int(pdfinfo_from_bytes(pdf_bytes).get("Pages", 0)) or None
        except Exception:
            doc_total = None  # banner degrades to "IMG n" without the total

    if page_numbers:
        page_images = []
        for p in page_numbers:
            # some poppler/pdf2image versions raise on out-of-range pages; skip rather than fail the request
            try:
                imgs = convert_from_bytes(pdf_bytes, dpi=dpi, fmt="PNG", first_page=p, last_page=p)
            except Exception as e:
                logger.warning(f"Page {p} conversion failed, skipping: {e}")
                continue
            if imgs:
                page_images.append((p, imgs[0]))
        if not page_images:
            raise HTTPException(status_code=422, detail="None of the requested pages could be converted")
    else:
        try:
            imgs = convert_from_bytes(pdf_bytes, dpi=dpi, fmt="PNG")
        except Exception as e:
            logger.error(f"PDF conversion failed: {e}")
            raise HTTPException(status_code=422, detail=f"PDF conversion failed: {str(e)}")
        page_images = list(enumerate(imgs, start=1))

    encoded = []
    for page_num, img in page_images:
        if img.width > MAX_DIM or img.height > MAX_DIM:
            # thumbnail before stamping so banner text stays crisp; labeled output may exceed MAX_DIM in height
            img.thumbnail((MAX_DIM, MAX_DIM), Image.LANCZOS)
        if label:
            img = _stamp(img, page_num, doc_total)
        buf = io.BytesIO()
        img.save(buf, format="PNG")
        encoded.append(base64.b64encode(buf.getvalue()).decode("utf-8"))

    logger.info(
        f"Converted {len(encoded)} pages from {len(pdf_bytes)} byte PDF "
        f"(dpi={dpi}, pages={pages_param or 'all'}, label={'on' if label else 'off'})"
    )
    result = {"images": encoded, "pages": len(encoded)}
    if page_numbers:
        result["page_numbers"] = [p for p, _ in page_images]
    return result


@app.get("/health")
async def health():
    return {"status": "ok"}
