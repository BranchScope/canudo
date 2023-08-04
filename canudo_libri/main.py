from pyrogram import Client, filters
from pyrogram.types import Message, CallbackQuery, InlineKeyboardMarkup, InlineKeyboardButton
from database import Database
from strings import STRINGS as strings
import os, asyncio, phonenumbers

API_ID, API_HASH, BOT_TOKEN, LOG_CHANNEL, ER_CANALO = os.getenv('API_ID'), os.getenv('API_HASH'), os.getenv('BOT_TOKEN'), int(os.getenv('LOG_CHANNEL')), int(os.getenv('ER_CANALO'))
db = Database()
pfx = ["!", ";", ".", ",", "-", "?", "*", "+", "#", "~", "_", "^", "/", ">"]
app = Client("picciottocanudobot", api_id=API_ID, api_hash=API_HASH, bot_token=BOT_TOKEN)

#mega pippone for check banned users and add/log new ones
@app.on_message(filters.private)
async def check_banana_message(client, message):
    banned = await db.is_banned(message.from_user.id)
    if not banned:
        checku = await db.check_user(message.from_user.id)
        if not checku:
            lang_code = "it" #hardcoded for now, because if i think that english would only be used by me and prof Cisternino :D
            addu = await db.add_user(message.from_user.id, message.from_user.first_name, message.from_user.last_name, message.from_user.username, lang_code)
            if addu:
                last_name = f"{message.from_user.last_name}" if message.from_user.last_name else ""
                username = f"<b>Username:</b> @{message.from_user.username}\n" if message.from_user.username else ""
                dc_id = f"<b>DC_ID:</b> <code>{message.from_user.dc_id}</code>" if message.from_user.dc_id else ""
                logs = f"#ADD #USER\n\n<b>Name:</b> <code>{message.from_user.first_name} {last_name}</code>\n{username}<b>ID:</b> <code>{message.from_user.id}</code>\n{dc_id}"
                await app.send_message(LOG_CHANNEL, logs)
        await message.continue_propagation()

#the freaking frucking terroning start
@app.on_message(filters.command("start", pfx) & filters.private)
async def start_command(client: Client, message: Message):
    await db.set_status(message.from_user.id, "")
    keyboard = InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton(strings["write_ad_btn"], callback_data="write_ad")
            ],
            [
                InlineKeyboardButton(strings["source_code_btn"], url="https://github.com/BranchScope/canudo")
            ]
        ]
    )
    await message.reply_text(strings["start"], reply_markup=keyboard, disable_web_page_preview=True)
    await message.continue_propagation()

#set_* handler via normal messages
@app.on_message(filters.private)
async def set_bullshit_handler(client: Client, message: Message):
    status = await db.get_status(message.from_user.id)
    if "set_bookname" in status:
        ad_id = status.split(":")[1].split("/")[0]
        await db.update_ad(ad_id, "book", message.text)
        await db.set_status(message.from_user.id, f"ad:{ad_id}")
        buttons = [[InlineKeyboardButton(f"{i} 🔘", f"trigger_year:{i}")] for i in range(1, 6)] #hardcoded to false because it's the first and unique send_message
        buttons.append([InlineKeyboardButton(strings["done"], callback_data="years_ok")])
        buttons.append([InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")])
        keyboard = InlineKeyboardMarkup(buttons)
        await message.reply_text(strings["ad_years_selection"], reply_markup=keyboard)
    elif "set_bookcode" in status:
        ad_id = status.split(":")[1].split("/")[0]
        await db.update_ad(ad_id, "book_code", message.text)
        await db.set_status(message.from_user.id, f"ad:{ad_id}/set_contacts")
        keyboard = InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")
                ]
            ]
        )
        await message.reply_text(strings["ad_contacts_imposteiscion"], reply_markup=keyboard)
    elif "set_contacts" in status:
        ad_id = status.split(":")[1].split("/")[0]
        try:
            contacts = message.text.splitlines()
            c_array = []
            for contact in contacts: #very bad stuffs here, please don't try this at home
                social = contact.split(" - ")[0]
                url = contact.split(" - ")[1]
                if "@" in url:
                    social = url
                    url = ""
                #I mean, if you are the king of regexes and you can help me, open a PR ¯\_(ツ)_/¯
                try:
                    mh = phonenumbers.parse(url, "IT")
                    if phonenumbers.is_valid_number(mh):
                        social = url
                        url = f"tel:{url}"
                    elif "numero" in social.lower():
                        social = url
                        url = f"tel:{url}"
                except:
                    if "numero" in social.lower():
                        social = url
                        url = f"tel:{url}"
                c_array.append([social, url])
            await db.update_ad(ad_id, "contacts", c_array)
            await db.set_status(message.from_user.id, f"ad:{ad_id}")
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["send_ad_btn"], callback_data="send_ad")
                    ],
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")
                    ]
                ]
            )
            await message.reply_text(strings["done_send"], reply_markup=keyboard)
        except Exception as e:
            print(e)
            await message.reply_text(strings["ad_contacts_imposteiscion_error"])
    await message.continue_propagation()

#forse in amore le rose non si usano più
@app.on_callback_query()
async def check_banana_query(client, query):
    banned = await db.is_banned(query.from_user.id)
    if not banned:
        await query.continue_propagation()

#callbackquery handler
@app.on_callback_query()
async def callbacks_handler(client: Client, query: CallbackQuery):
    status = await db.get_status(query.from_user.id)
    if query.data == "write_ad":
        ad_id = await db.create_ad(query.from_user.id)
        await db.set_status(query.from_user.id, f"ad:{ad_id}")
        subjects = await db.get_subjects()
        buttons = [[InlineKeyboardButton(x['name'], f"set_ad_subject:{x['code_name']}")] for x in subjects]
        buttons.append([InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")])
        keyboard = InlineKeyboardMarkup(buttons)
        await query.message.edit(strings["ad_subject_selection"], reply_markup=keyboard)
        await query.answer()
    if "ad" in status: #that's cringe though
        ad_id = status.split(":")[1].split("/")[0]
        if "set_ad_subject" in query.data:
            subject = query.data.split(":")[-1]
            await db.update_ad(ad_id, "subject", subject)
            await db.set_status(query.from_user.id, f"{status}/set_bookname") #from bad to badder (cit.)
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")
                    ]
                ]
            )
            await query.message.edit(strings["ad_bookname_imposteiscion"], reply_markup=keyboard)
            await query.answer()
        elif "trigger_year" in query.data: #if it works, don't touch it (even if the codebase makes Topolino put his hands in his eyes)
            ad = await db.get_ad_by_id(ad_id)
            years = [year for year in ad['years']] if ad['years'] is not None else []
            year = int(query.data.split(":")[1])
            years.append(year) if year not in years else years.remove(year)
            years.sort() #e mamt
            await db.update_ad(ad_id, "years", years)
            ad = await db.get_ad_by_id(ad_id)
            years = ad['years']
            buttons = [[InlineKeyboardButton(f"{i} {'🟢' if i in years else '🔘'}", f"trigger_year:{i}")] for i in range(1, 6)]
            buttons.append([InlineKeyboardButton(strings["done"], callback_data="years_ok")])
            buttons.append([InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")])
            keyboard = InlineKeyboardMarkup(buttons)
            await query.message.edit(strings["ad_years_selection"], reply_markup=keyboard)
            await query.answer()
        elif query.data == "years_ok":
            await db.set_status(query.from_user.id, f"{status}/set_bookcode")
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["skip_set_bookcode_btn"], callback_data="skip_set_bookcode")
                    ],   
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")
                    ]
                ]
            )
            await query.message.edit(strings["ad_bookcode_imposteiscion"], reply_markup=keyboard)
            await query.answer()
        elif query.data == "skip_set_bookcode":
            await db.set_status(query.from_user.id, f"ad:{ad_id}/set_contacts")
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}")
                    ]
                ]
            )
            await query.message.edit(strings["ad_contacts_imposteiscion"], reply_markup=keyboard)
        elif query.data == "send_ad":
            await db.set_status(query.from_user.id, "")
            ad = await db.get_ad_by_id(ad_id)
            contacts = []
            for social, url in ad['contacts']:
                contacts.append(f"<a href='{url}'>{social}</a>")
            #and the award for the worst line of code goes to:
            txt = strings["ad"].replace("{BOOK_NAME}", ad['book']).replace("{SUBJECT}", (await db.get_subject(ad['subject']))).replace("{YEARS}", '°, '.join(str(year) for year in ad['years']) + "°" if ad['years'] is not None else "Non definito").replace("{CONTACTS}", ', '.join(contacts)).replace("{FROM_USER_ID}", str(ad['from_user'])).replace("{FROM_USER_NAME}", (await db.get_user(ad['from_user']))['first_name']).replace("{BOOK_CODE_STR}", f"<b>Codice libro:</b> <code>{ad['book_code']}</code>\n" if ad['book_code'] else "")
            msg = await app.send_message(ER_CANALO, txt)
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["url_btn"], url=f"t.me/c/{abs(ER_CANALO + int(1e12))}/{msg.id}")
                    ],
                    [
                        InlineKeyboardButton(strings["cancel_ad_btn"], callback_data=f"cancel_ad/ad_id:{ad_id}/message_id:{msg.id}")
                    ]
                ]
            )
            yay = await query.message.edit(strings["congrats"], reply_markup=keyboard)
            await query.answer()
            await yay.pin(both_sides=True)
    if "cancel_ad" in query.data:
        p = query.data.split("/")
        ad_id = int(p[1].split(":")[1])
        ad = await db.get_ad_by_id(ad_id)
        if ad['from_user'] == query.from_user.id: #cybersecurity expert stuffs here
            await db.set_status(query.from_user.id, "")
            await db.delete_ad(ad_id)
            if "message_id" in query.data:
                await app.delete_messages(ER_CANALO, int(p[2].split(":")[1]))
            keyboard = InlineKeyboardMarkup(
                [
                    [
                        InlineKeyboardButton(strings["write_ad_btn"], callback_data="write_ad")
                    ],
                    [
                        InlineKeyboardButton(strings["source_code_btn"], url="https://github.com/BranchScope/canudo")
                    ]
                ]
            )
            nop = await query.message.edit(strings["start"], reply_markup=keyboard, disable_web_page_preview=True)
            await query.answer()
            await nop.unpin()

#ADMIN RELATED STUFFS, FATTI LI CAZZI TUA <3
@app.on_message(filters.command("magic", pfx) & filters.private)
async def magical_tests(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        await message.reply_text("yeah my man", quote=True)
    await message.continue_propagation()

@app.on_message(filters.command("subs", pfx) & filters.private)
async def subs_command(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        subs = await db.get_subs()
        await message.reply_text(f"📊 <b>Total users</b>\n{subs}", quote=True)
    await message.continue_propagation()


@app.on_message(filters.command(["post"], pfx) & filters.private)
async def post(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        regex = r"(?s)(?<=post )(.*$)"
        matches = re.search(regex, message.text)
        if matches:
            text = matches.group()
        users = await db.get_users()
        for user in users:
            try:
                await app.send_message(user['user_id'], text, disable_web_page_preview=True)
            except Exception as e:
                print(e)
    await message.continue_propagation()

@app.on_message(filters.command(["ban"], pfx) & filters.private)
async def ban_cmd(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        if len(message.command) == 2:
            if message.command[1].isdigit():
                ban = await db.ban_user(message.command[1])
                if ban is True:
                    await message.reply_text("User banned!", quote=True)
                else:
                    await message.reply_text("User not found.", quote=True)
            else:
                await message.reply_text("Invalid user ID.", quote=True)
        else:
            await message.reply_text("Are you dumb or like what?", quote=True)
    await message.continue_propagation()

@app.on_message(filters.command(["unban"], pfx) & filters.private)
async def unban_cmd(client: Client, message: Message):
    rank = await db.get_user_rank(message.from_user.id)
    if rank > 0:
        if len(message.command) == 2:
            if message.command[1].isdigit():
                unban = await db.unban_user(message.command[1])
                if unban is True:
                    await message.reply_text("User unbanned!", quote=True)
                else:
                    await message.reply_text("User not found.", quote=True)
            else:
                await message.reply_text("Invalid user ID.", quote=True)
        else:
            await message.reply_text("Are you dumb or like what?", quote=True)
    await message.continue_propagation()

#the engine starter
async def main():
    await db.connect()
    await app.start()
    await asyncio.Future() #sleep forever zZzZ...

app.run(main())
