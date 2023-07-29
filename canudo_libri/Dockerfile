FROM python:latest
RUN pip install --no-cache-dir pyrogram tgcrypto asyncpg asyncio phonenumbers
COPY . .
ENTRYPOINT ["python", "main.py"]