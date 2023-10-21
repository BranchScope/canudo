# DEPLOY GUIDE
Ok, I mean, I dockerized it for a reason...

## Installation & requirements
- Install python3 and docker on your system
- `git clone https://github.com/BranchScope/canudo`
- `pip install -r requirements.txt` inside this magic directory (`~/canudo/canudo_libri/`)

## Start
- `docker-compose up -d`

## Logging
- `docker-compose logs` or run the `up` command without the flag `-d` (just in case you didn't know how to use docker lmao)