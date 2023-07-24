# DEPLOY GUIDE
Ok, I mean, I dockerized it for a reason...

## Installation & requirements
- Install docker for your system
- `git clone https://github.com/BranchScope/canudo`

## Start
- `cd canudo`
- `docker-compose build`
- `docker-compose up -d database`
- `docker-compose up -d bot` (I hate health checks)
I don't actually know how to set up this docker container in the right way, so don't hesitate to make PR if you want...

## Logging
- `docker-compose logs` or run the `up` command without the flag `-d` (just in case you didn't know how to use docker lmao)