from pyrogram import Client, filters
from pyrogram.types import Message, CallbackQuery, InlineKeyboardMarkup, InlineKeyboardButton
from config import API_ID, API_HASH, BOT_TOKEN, LOG_CHANNEL, ER_CANALO
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
            addu = await db.add_user(message.from_user.id, message.from_user.first_name, message.from_user.last_name, message.from_user.username, lang_code)
            if addu is True:
                last_name = f"{message.from_user.last_name}" if message.from_user.last_name else ""
                username = f"<b>Username:</b> @{message.from_user.username}\n" if message.from_user.username else ""
                dc_id = f"<b>DC_ID:</b> <code>{message.from_user.dc_id}</code>" if message.from_user.dc_id else ""
                logs = f"#ADD #USER\n\n<b>Name:</b> <code>{message.from_user.first_name} {last_name}</code>\n{username}<b>ID:</b> <code>{message.from_user.id}</code>\n{dc_id}"
                await app.send_message(LOG_CHANNEL, logs)
        await message.continue_propagation()

#the freaking frucking terroning start
@app.on_message(filters.command("start", ["/"]) & filters.private)
async def start_command(client: Client, message: Message):
    await db.set_status(message.from_user.id, "")
    keyboard = InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton(strings["write_ad_btn"], callback_data="write_ad")
            ]
        ]
    )
    await message.reply_text(strings["start"], reply_markup=keyboard)
    await message.continue_propagation()

#set_bookname handler
@app.on_message(filters.private)
async def set_bookname_handler(client: Client, message: Message):
    status = await db.get_status(message.from_user.id)
    if "set_bookname" in status:
        ad_id = status.split(":")[1].split("/")[0]
        await db.update_ad(ad_id, "book", message.text)
        await db.set_status(message.from_user.id, f"ad:{ad_id}")
        buttons = [[InlineKeyboardButton(f"{i} 🔘", f"trigger_year:{i}")] for i in range(1, 6)] #hardcoded to false because it's the first and unique send_message
        buttons.append([InlineKeyboardButton(strings["done"], callback_data="years_ok")])
        keyboard = InlineKeyboardMarkup(buttons)
        await message.reply_text(strings["ad_years_selection"], reply_markup=keyboard)
    elif "set_contacts" in status:
        ad_id = status.split(":")[1].split("/")[0]
        try:
            contacts = message.text.splitlines()
            c_array = []
            for contact in contacts:
                social = contact.split(" - ")[0]
                url = contact.split(" - ")[1]
                c_array.append([social, url])
            await db.update_ad(ad_id, "contacts", c_array)
            await db.set_status(message.from_user.id, f"ad:{ad_id}")
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["send_ad_btn"], callback_data="send_ad")
                    ],
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data="cancel_ad")
                    ]
                ]
            )
            await message.reply_text(strings["done_send"], reply_markup=keyboard)
        except Exception as e:
            print(e)
            await message.reply_text(strings["ad_contacts_imposteiscion_error"])
    await message.continue_propagation()

#callbackquery handler
@app.on_callback_query()
async def callbacks_handler(client: Client, query: CallbackQuery):
    status = await db.get_status(query.from_user.id)
    if query.data == "write_ad":
        ad_id = await db.create_ad(query.from_user.id)
        await db.set_status(query.from_user.id, f"ad:{ad_id}")
        subjects = await db.get_subjects()
        buttons = [[InlineKeyboardButton(x['name'], f"set_ad_subject:{x['code_name']}")] for x in subjects]
        keyboard = InlineKeyboardMarkup(buttons)
        await query.message.edit(strings["ad_subject_selection"], reply_markup=keyboard)
        await query.answer()
    if "ad" in status: #that's cringe though
        ad_id = status.split(":")[1].split("/")[0]
        if "set_ad_subject" in query.data:
            subject = query.data.split(":")[-1]
            await db.update_ad(ad_id, "subject", subject)
            await db.set_status(query.from_user.id, f"{status}/set_bookname") #from bad to badder (cit.)
            await query.message.edit(strings["ad_bookname_imposteiscion"])
            await query.answer()
        elif "trigger_year" in query.data: #if it works, don't touch it (even if the codebase makes Topolino put his hands in his eyes)
            ad = await db.get_ad_by_id(ad_id)
            years = [year for year in ad['years']] if ad['years'] is not None else []
            year = int(query.data.split(":")[1])
            years.append(year) if year not in years else years.remove(year)
            await db.update_ad(ad_id, "years", years)
            ad = await db.get_ad_by_id(ad_id)
            years = ad['years']
            buttons = [[InlineKeyboardButton(f"{i} {'🟢' if i in years else '🔘'}", f"trigger_year:{i}")] for i in range(1, 6)]
            buttons.append([InlineKeyboardButton(strings["done"], callback_data="years_ok")])
            keyboard = InlineKeyboardMarkup(buttons)
            await query.message.edit(strings["ad_years_selection"], reply_markup=keyboard)
            await query.answer()
        elif query.data == "years_ok":
            await db.set_status(query.from_user.id, f"{status}/set_contacts")
            await query.message.edit(strings["ad_contacts_imposteiscion"])
            await query.answer()
        elif query.data == "send_ad":
            await db.set_status(query.from_user.id, "")
            ad = await db.get_ad_by_id(ad_id)
            #parsing stuffs
            msg = await app.send_message(ER_CANALO, ad["book"]) #just for testing, i'll continue it later ma friend so stanco
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["url_btn"], url=f"t.me/c/{abs(ER_CANALO + int(1e12))}/{msg.id}")
                    ]
                ]
            )
            await query.message.edit(strings["congrats"], reply_markup=keyboard)
            await query.answer()
#ADMIN RELATED STUFFS, FATTI LI CAZZI TUA <3
@app.on_message(filters.command("magic", ["/"]) & filters.private)
async def magical_tests(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        await message.reply_text("yeah my man", quote=True)
    await message.continue_propagation()

#the engine starter
async def main():
    await db.connect()
    await app.start()
    await asyncio.Future() #sleep forever zZzZ...

app.run(main())
