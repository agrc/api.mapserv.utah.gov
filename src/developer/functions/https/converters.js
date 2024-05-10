import { timeSince } from '../time.js';
const dateFormatter = new Intl.DateTimeFormat('en-US', {
  year: 'numeric',
  month: 'numeric',
  day: 'numeric',
  hour: 'numeric',
  minute: 'numeric',
  timeZone: 'MST',
});

export const standardKeyConversion = {
  toFirestore(data) {
    return data;
  },
  fromFirestore(snapshot, options) {
    const data = snapshot.data(options);

    if (!data.created) {
      data.created = { toDate: () => new Date() };
    }

    return {
      key: data.key.toUpperCase(),
      created: timeSince(Date.parse(data.created.toDate().toISOString())),
      createdDate: dateFormatter.format(
        Date.parse(data.created.toDate().toISOString()),
      ),
      pattern: data.pattern,
      flags: data.flags,
      notes: data.notes ?? 'edit this key to add notes',
    };
  },
};
