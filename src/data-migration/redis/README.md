# Redis migration

## Get the data

1. Use the `redis-cli save` command to export a `dump.rdb`.
1. Place it in this folder

## Setup python

1. Use venv to create a virtual env.
   - `python -m venv pyenv`
1. Install the packages
   - source ./pyenv/bin/activate
   - pip install -r requirements.txt

## Extract from the RDB

1. Covert the redis dump to key value pairs
   - rdb --command justkeyvals ./dump.rdb | sort > dump.txt

## Filter the data

## Load the data
