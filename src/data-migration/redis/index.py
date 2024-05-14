import redis

def parse_dump():
  keys = {}
  with open('dump.txt', 'r') as data:
    for entry in data:
      entry = entry.strip().rstrip(',')

      if len(entry) == 0:
        continue

      key, value = entry.split(' ')

      #: entries without a colon are total values
      if ':' not in key:
        method = 'total'
      else:
        key, method = key.split(':')

      key = key.lower().strip()
      method = method.lower().strip()

      if method not in ['total', 'time', 'geocode', 'search', 'info']:
        print('skipping ', method)

        continue

      if key not in keys:
        keys[key] = {'analytics:hit': 0, 'analytics:time': 0, 'analytics:geocode': 0, 'analytics:search': 0, 'analytics:info': 0}

      if method == 'total':
          keys[key]['analytics:hit'] = keys[key]['analytics:hit'] + int(value)
      elif method == 'time':
        keys[key]['analytics:time'] = max(keys[key]['analytics:time'], int(value))
      elif method == 'geocode':
        keys[key]['analytics:geocode'] = keys[key]['analytics:geocode'] + int(value)
      elif method == 'search':
        keys[key]['analytics:search'] = keys[key]['analytics:search'] + int(value)
      elif method == 'info':
        keys[key]['analytics:info'] = keys[key]['analytics:info'] + int(value)


  return keys

keys = parse_dump()

r = redis.Redis()
pipe = r.pipeline()

for key, values in keys.items():
  for method, value in values.items():
    redis_key = f"{method}:{key}"

    if value > 0:
      pipe.set(redis_key, str(value))
    else:
      pipe.delete(redis_key)

pipe.execute()
