import requests, re, urllib, json
from bs4 import BeautifulSoup

URL = "https://www.canudo.edu.it/index.php"
page = requests.get(URL)
soup = BeautifulSoup(page.content, "html.parser")
job_elements = soup.find_all("div", class_="items-leading clearfix")

def check_last_com(last_com):
    with open("last.txt", "r") as f:
        last = f.read()
    if last == last_com:
        return True
    else:
        return False

for job_element in job_elements:
    try:
        com = job_element.find("h2", class_="item-title").text.lstrip()
        if check_last_com(com):
            break
        with open("last.txt", "w") as f:
            f.write(com)
        link = "https://www.canudo.edu.it" + urllib.parse.quote(job_element.find_all("a")[0]["href"])
        try:
            uptitle = job_element.find("p", style="text-align: justify;").text
            if "Oggetto" in uptitle:
                title = uptitle
            else:
                title = ""
        except:
            title = ""
        keyboard = {"inline_keyboard": [[{"text": "📎 Balza sul sito", "url": link}]]}
        requests.post("https://api.telegram.org/bot5171831004:AAH89EXSzyThNSiHuAy8oRSi-qHLAStxi6o/sendMessage", data={"chat_id": -1001650476739, "text": "<b>📍" + com + "</b>" + title, "reply_markup": json.dumps(keyboard), "parse_mode": "HTML"})
    except:
        print("error")
