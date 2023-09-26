const pluralRules = new Intl.PluralRules('en-US');

const pluralize = (count, singular, plural) => {
  const grammaticalNumber = pluralRules.select(count);
  switch (grammaticalNumber) {
    case 'one':
      return `${count} ${singular}`;
    case 'other':
      return `${count} ${plural}`;
    default:
      throw new Error('Unknown: ' + grammaticalNumber);
  }
};

export const timeSince = (date) => {
  const seconds = Math.floor((new Date() - date) / 1000);

  let interval = seconds / 31536000;

  if (interval > 1) {
    return `${pluralize(Math.floor(interval), 'yr', 'yrs')} ago`;
  }
  interval = seconds / 2592000;
  if (interval > 1) {
    return `${Math.floor(interval)} mo ago`;
  }
  interval = seconds / 86400;
  if (interval > 1) {
    return `${pluralize(Math.floor(interval), 'day', 'days')} ago`;
  }
  interval = seconds / 3600;
  if (interval > 1) {
    return `${pluralize(Math.floor(interval), 'hr', 'hrs')} ago`;
  }
  interval = seconds / 60;
  if (interval > 1) {
    return `${pluralize(Math.floor(interval), 'min', 'mins')} ago`;
  }

  return `${Math.floor(seconds)} s ago`;
};