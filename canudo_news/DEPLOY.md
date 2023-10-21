# DEPLOY GUIDE
This just uses a crontab, simple innit?

## Installation & requirements
- Install python3 on your system
- `git clone https://github.com/BranchScope/canudo`
- `pip install -r requirements.txt` inside this magic directory (`~/canudo/canudo_news/`)

## Start
- `(crontab -l 2>/dev/null; echo "*/2 * * * * /path/to/canudo/canudo_news && /usr/bin/python3 /path/to/canudo/canudo_news/main.py") | crontab -`\
(yeah it's such a long command, i think it's the best copy&paste solution for now)

## Logging
- well... start the main.py manually and read the stdout :D