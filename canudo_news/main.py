import requests, re, urllib, json
from bs4 import BeautifulSoup

URL = "https://www.canudo.edu.it"
TOKEN = "REDACTED"
CHAT_ID = -1001848463147

def send_message(chat_id: int, text: str, keyboard: dict | None) -> str:
    api_url = f"https://api.telegram.org/bot{TOKEN}/sendMessage"
    data = {
        "chat_id": chat_id,
        "text": text,
        "parse_mode": "HTML"
    }

    if keyboard:
        data["reply_markup"] = json.dumps(keyboard)
    
    return requests.post(api_url, data=data).text

def scrape_com():
    #setup of the grand theft news technologies
    #it's actually a big salad of random bullshit wrote togheter, I don't even have the head to order that
    page = requests.get(URL + "/index.php")
    soup = BeautifulSoup(page.content, "html.parser")
    first_job_element = soup.find("div", class_="items-leading clearfix")
    others_job_elements = soup.find_all("div", class_="item column-1 span12")
    link_array = [URL + urllib.parse.quote(job_element.find_all("a")[0]["href"]) for job_element in others_job_elements]
    link_array.reverse()
    link_array.append(URL + urllib.parse.quote(first_job_element.find_all("a")[0]["href"]))
    with open("last.txt", "r") as f:
        content = f.read().splitlines(True)
    inception = [i for i in link_array if i + "\n" not in content] #https://stackoverflow.com/a/740294/14426239 (reversed)
    if inception == []:
        raise Exception("Everything already sent.")
    for link in inception:
        page = requests.get(link)
        soup = BeautifulSoup(page.content, "html.parser")
        try:
            com = soup.find("div", class_="page-header").text.lstrip()

            """removed the limit of 5, so..."""
            #Decisions as I go, to anywhere I flow
            #Sometimes I believe, at times I'm rational
            #I can fly high, I can go low
            """Today I got a million, tomorrow, I don't know"""
            with open("last.txt", "w") as f:
                f.writelines(content[1:])
            
            with open("last.txt", "a") as f:
                f.write(link + "\n")

            try:
                uptitle = soup.find("p", style="text-align: justify;").text
                if uptitle and "Oggetto" in uptitle:
                    title = uptitle.lstrip()
                else:
                    title = soup.find("span", style="font-size: 12pt; font-family: arial, helvetica, sans-serif;").text.lstrip()
                title = title.replace("Oggetto - ", "")
            except: #oneline if-statements are so boring, just try and catch everything my bud
                title = ""
                
            keyboard = {"inline_keyboard": [[{"text": "📎 Balza sul sito", "url": link}]]}
            print(send_message(CHAT_ID, f"<b>📍{com} </b> <i>{title}</i>", keyboard))
        except Exception as e:
            raise Exception(f"Come a little closer, then you'll see (come on, come on, come on): {e}")

scrape_com() #*"Can You Hear The Music" by Ludwig Göransson starts playing in background*
