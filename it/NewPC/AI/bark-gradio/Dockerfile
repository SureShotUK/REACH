FROM niteshbalusu/bark:latest

WORKDIR /workspace

RUN pip install gradio soundfile

COPY app.py .

EXPOSE 7861

CMD ["python", "app.py"]
