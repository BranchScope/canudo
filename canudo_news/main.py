import requests, re, urllib, json
from bs4 import BeautifulSoup

URL = "https://www.canudo.edu.it"
TOKEN = "REDACTED"
page = requests.get(URL + "/index.php")

soup = BeautifulSoup(page.content, "html.parser")
job_elements = soup.find_all("div", class_="items-leading clearfix")

def send_message(chat_id: int, text: str, keyboard: dict | None) -> dict:
    api_url = f"https://api.telegram.org/bot{TOKEN}/sendMessage"
    data = {
        "chat_id": chat_id,
        "text": text,
        "parse_mode": "HTML"
    }

    if keyboard:
        data["reply_markup"] = json.dumps(keyboard)
    
    return requests.post(api_url, data=data).text

for job_element in job_elements:
    try:
        com = job_element.find("h2", class_="item-title").text.lstrip()
        link = URL + urllib.parse.quote(job_element.find_all("a")[0]["href"])

        with open("last.txt", "r") as f:
            content = f.read().splitlines(True)

        if content[-1] == link + "\n":
            break

        if len(content) >= 4:
            with open("last.txt", "w") as f:
                f.writelines(content[1:])
        
        with open("last.txt", "a") as f:
            f.write(link + "\n")

        try:

            uptitle = job_element.find("p", style="text-align: justify;").text
            if uptitle and "Oggetto" in uptitle:
                title = uptitle.lstrip()
            else:
                title = job_element.find("span", style="font-size: 12pt; font-family: arial, helvetica, sans-serif;").text.lstrip()
            title = title.replace("Oggetto - ", "")
        except: #oneline if-statements are so boring, just try and catch everything my bud
            title = ""
            
        keyboard = {"inline_keyboard": [[{"text": "📎 Balza sul sito", "url": link}]]}
        print(send_message(1876496621, f"<b>📍{com} </b> <i>{title}</i>", keyboard))
    except Exception as e:
        print(f"Come a little closer, then you'll see (come on, come on, come on): {e}")
