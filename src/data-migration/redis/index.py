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
        keys[key] = {'total': 0, 'time': 0, 'geocode': 0, 'search': 0, 'info': 0}

      if method == 'total':
          keys[key]['total'] = keys[key]['total'] + int(value)
      elif method == 'time':
        keys[key]['time'] = max(keys[key]['time'], int(value))
      elif method == 'geocode':
        keys[key]['geocode'] = keys[key]['geocode'] + int(value)
      elif method == 'search':
        keys[key]['search'] = keys[key]['search'] + int(value)
      elif method == 'info':
        keys[key]['info'] = keys[key]['info'] + int(value)


  return keys

keys = parse_dump()

r = redis.Redis()
pipe = r.pipeline()

for key, values in keys.items():
  for method, value in values.items():
    redis_key = f"{key}:{method}"

    if method == 'total':
      redis_key = key

    if value > 0:
      pipe.set(redis_key, str(value))
    else:
      pipe.delete(redis_key)

pipe.execute()
