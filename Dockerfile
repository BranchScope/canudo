FROM python:latest
RUN pip install pyrogram tgcrypto asyncpg asyncio phonenumbers
ADD . .
ENTRYPOINT ["python", "main.py"]