// eslint-disable-next-line no-undef
module.exports.migrate = async ({ firestore }) => {
  const generateRegexFromPattern = (await import('../functions/https/createKey.js')).generateRegexFromPattern;

  const keysSnapshot = await firestore.collectionGroup('keys').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  for (const doc of keysSnapshot.docs) {
    const key = doc.data();

    // server keys are matched by ip address and do not use a regular expression
    if (key.flags?.server !== false) {
      continue;
    }

    size += 1;

    // regenerate the regular expression from the user pattern so every regex metacharacter is escaped and the host is anchored
    const regularExpression = generateRegexFromPattern(key.pattern);
    const update = { regularExpression };

    // a pattern that can not be expressed safely must not match any referrer
    if (regularExpression === '') {
      update['flags.disabled'] = true;
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
};
