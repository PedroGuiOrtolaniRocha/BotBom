import json.scanner
from playwright.sync_api import sync_playwright
import time
import json
import pyperclip

def get_json():
    with open('./config.json','r') as archive:
        
        data = json.load(archive)
        return data

def set_json(data : dict):
    with open('config.json', 'w') as archive:
        json.dump(data,archive,indent=2)

def run():

    config_data = get_json()
    email = config_data["email"]
    password = config_data["password"]
    username = config_data["userName"]

    time.sleep(1)
    
    with sync_playwright() as playwright:

        browser = playwright.chromium.launch(headless=False)
        context = browser.new_context(
            viewport={'width': 1920, 'height': 1080},
            device_scale_factor=1
        )

        page = context.new_page()
        page.goto("https://discord.com/developers/applications")

        page.fill('input[name="email"]', email)
        page.fill('input[name="password"]', password)

        button_selector = 'button[type="submit"]'
        button = page.wait_for_selector(button_selector, timeout=10000)
        
        if button:
            button.click()

        time.sleep(3)

        cards = page.query_selector_all('div[class="card-3JP0Fm"]')

        has_bot = False

        print(len(cards))
        if len(cards) != 0:
        
            has_bot = True
            page.wait_for_selector('.cardImage-21AQtd')
            page.click('.cardImage-21AQtd')
        
        if not has_bot:
            
            button_selector = page.wait_for_selector('button[type="button"][class*="button-f2h6uQ"]:has(span:text("New Application"))')
            button_selector.click()

            time.sleep(2)

            page.wait_for_selector('input[class*="inputDefault-3FGxgL"][type="text"]')
            page.fill('input[class*="inputDefault-3FGxgL"][type="text"]', username)

            page.check('input[type="checkbox"]')

            page.click('button[type="submit"]')

        page.click('button[class="button-f2h6uQ filledBrand-3fai8P filledDefault-25rIra buttonHeightShort-5JiyOY unpaired-GdFe-D copyAction-3ReaM0"]:has(span:text("Copy"))')
        bot_id = pyperclip.paste()

        while(page.url == "https://discord.com/developers/applications"):
            time.sleep(1)
            print(page.url)
            page.reload()
        
        print(page.url)
        
        if page.url.endswith("information"):
            page.goto(page.url.replace("information","bot"))

        else:
            page.goto(page.url +"/bot")

        time.sleep(3)
        page.wait_for_selector("input[name='botPublic']")
        page.check("input[name='botPublic']")

        time.sleep(3)
        checkboxes = page.query_selector_all('input[type="checkbox"]')

        if len(checkboxes) >= 3:
            checkboxes[2].check()  # Segundo checkbox

        if len(checkboxes) >= 4:
            checkboxes[3].check()  # Terceiro checkbox

        if len(checkboxes) >= 5:
            checkboxes[4].check()  # Quarto checkbox

        name = "input[name='username']"
        page.fill(name, username)
        
        time.sleep(2)

        save_button_selector = 'button[type="button"][class*="filledGreen"]:has(span:text("Save Changes"))'
        page.wait_for_selector(save_button_selector)

        page.click(save_button_selector)
        
        reset_button_selector = 'button[type="button"][class*="button-f2h6uQ"]'
        page.click(reset_button_selector)

        time.sleep(2)

        div_selector = 'div[class="modalContent-31OOYM"] button'
        buttons = page.query_selector_all(div_selector)
        for index, button in enumerate(buttons):
            if(button.inner_text() == "Yes, do it!"):
                button.click()
            
        passwd_selector = 'input[name="mfaCode"]'
        page.fill(passwd_selector, password)

        buttons = page.query_selector_all(div_selector)
        for index, button in enumerate(buttons):
            if(button.inner_text() == "Submit"):
                button.click()

        time.sleep(3)

        div_selector = 'div[class="modalContent-31OOYM"]'
        page.get_by_text("Copy").nth(1).click()

        token = pyperclip.paste()

        invite_url = f'https://discord.com/oauth2/authorize?client_id={bot_id}&permissions=8&integration_type=0&scope=bot'

        #return sets
        values = [token, invite_url]

        return values

values = run()
data = get_json()
data["token"] = values[0]
data["invite"] = values[1]
set_json(data)