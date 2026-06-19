from fastapi import FastAPI, Request, HTTPException
from pdf2image import convert_from_bytes
from PIL import Image
import base64
import io
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(title="PDF to Image Converter")

DPI = 200
MAX_DIM = 2000


@app.post("/convert")
async def convert_pdf(request: Request):
    pdf_bytes = await request.body()
    if not pdf_bytes:
        raise HTTPException(status_code=400, detail="No PDF data received")

    try:
        images = convert_from_bytes(
            pdf_bytes,
            dpi=DPI,
            fmt="PNG",
        )
    except Exception as e:
        logger.error(f"PDF conversion failed: {e}")
        raise HTTPException(status_code=422, detail=f"PDF conversion failed: {str(e)}")

    encoded = []
    for img in images:
        if img.width > MAX_DIM or img.height > MAX_DIM:
            img.thumbnail((MAX_DIM, MAX_DIM), Image.LANCZOS)
        buf = io.BytesIO()
        img.save(buf, format="PNG")
        encoded.append(base64.b64encode(buf.getvalue()).decode("utf-8"))

    logger.info(f"Converted {len(encoded)} pages from {len(pdf_bytes)} byte PDF")
    return {"images": encoded, "pages": len(encoded)}


@app.get("/health")
async def health():
    return {"status": "ok"}
