// eslint-disable-next-line no-undef
module.exports.migrate = async ({ firestore }) => {
  const generateRegexFromPattern = (await import('../functions/https/createKey.js')).generateRegexFromPattern;

  const keysSnapshot = await firestore.collectionGroup('keys').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;
  const disabled = [];

  for (const doc of keysSnapshot.docs) {
    const key = doc.data();

    // server keys are matched by ip address and do not use a regular expression
    if (key.flags?.server !== false) {
      continue;
    }

    // elevated keys bypass the referrer check entirely so their pattern is never used
    if (key.elevated === true) {
      continue;
    }

    size += 1;

    // regenerate the regular expression from the user pattern so every regex metacharacter is escaped and the host is anchored
    const regularExpression = generateRegexFromPattern(key.pattern);
    const update = { regularExpression };

    // a pattern that can not be expressed safely must not match any referrer
    if (regularExpression === '') {
      update['flags.disabled'] = true;
      disabled.push({ id: doc.id, pattern: key.pattern });
    }

    batches[batchIndex].update(doc.ref, update);

    if (size % 400 === 0) {
      batchIndex += 1;
      batches[batchIndex] = firestore.batch();
    }
  }

  for (const batch of batches) {
    await batch.commit();
  }

  // eslint-disable-next-line no-undef
  console.log(`Disabled ${disabled.length} of ${size} browser keys with patterns that can not be expressed safely`);
  for (const { id, pattern } of disabled) {
    // eslint-disable-next-line no-undef
    console.log(`Disabled key ${id} with pattern ${JSON.stringify(pattern)}`);
  }
};
