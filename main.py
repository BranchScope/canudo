from pyrogram import Client, filters
from pyrogram.types import Message, CallbackQuery, InlineKeyboardMarkup, InlineKeyboardButton
from config import API_ID, API_HASH, BOT_TOKEN, LOG_CHANNEL
from database import Database
from strings import STRINGS as strings
import asyncio

db = Database()
app = Client("picciottocanudobot", api_id=API_ID, api_hash=API_HASH, bot_token=BOT_TOKEN)

#mega pippone for check banned users and add/log new ones
@app.on_message(filters.private)
async def check_banana_message(client, message):
    banned = await db.is_banned(message.from_user.id)
    if banned is not True:
        checku = await db.check_user(message.from_user.id)
        if checku is False:
            lang_code = "it" #hardcoded for now, because if i think that english would only be used by me and prof Cisternino :D
            addu = await db.add_user(message.from_user.id, message.from_user.first_name, message.from_user.last_name, message.from_user.username, lang_code, "has_to_setup")
            if addu is True:
                last_name = f"{message.from_user.last_name}" if message.from_user.last_name else ""
                username = f"<b>Username:</b> @{message.from_user.username}\n" if message.from_user.username else ""
                dc_id = f"<b>DC_ID:</b> <code>{message.from_user.dc_id}</code>" if message.from_user.dc_id else ""
                logs = f"#ADD #USER\n\n<b>Name:</b> <code>{message.from_user.first_name} {last_name}</code>\n{username}<b>ID:</b> <code>{message.from_user.id}</code>\n{dc_id}"
                await app.send_message(LOG_CHANNEL, logs)
        await message.continue_propagation()

#the actual start
@app.on_message(filters.command("start", ["/"]) & filters.private)
async def start_command(client: Client, message: Message):
    await message.reply_text(strings["start"])
    await message.continue_propagation()

#magical tests for admins
@app.on_message(filters.command("magic", ["/"]) & filters.private)
async def magical_tests(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        await message.reply_text("yeah my man", quote=True)
    await message.continue_propagation()

async def main():
    await db.connect()
    await app.start()
    await asyncio.Future()  # sleep forever

app.run(main())