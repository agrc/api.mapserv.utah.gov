import { timeSince } from '../time.js';
const dateFormatter = new Intl.DateTimeFormat('en-US', {
  year: 'numeric',
  month: 'numeric',
  day: 'numeric',
  hour: 'numeric',
  minute: 'numeric',
  timeZone: 'MST',
});

export const minimalKeyConversion = {
  toFirestore(data) {
    return data;
  },
  fromFirestore(snapshot, options) {
    const data = snapshot.data(options);

    return {
      key: data.key,
      created: timeSince(Date.parse(data.created.toDate().toISOString())),
      createdDate: dateFormatter.format(
        Date.parse(data.created.toDate().toISOString()),
      ),
      notes: data.notes ?? 'edit this key to add notes',
    };
  },
};
