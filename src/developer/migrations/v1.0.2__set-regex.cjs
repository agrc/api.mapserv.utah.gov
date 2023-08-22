module.exports.migrate = async ({ firestore }) => {
  const generateRegexFromPattern = (
    await import('../functions/https/createKey.js')
  ).generateRegexFromPattern;

  const keysSnapshot = await firestore.collectionGroup('keys').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  await keysSnapshot.forEach(async (doc) => {
    size += 1;
    const key = doc.data();

    if (key.type !== 'browser') {
      return;
    }

    batches[batchIndex].update(doc.ref, {
      regularExpression: generateRegexFromPattern(key.pattern),
    });

    if (size % 400 === 0) {
      batchIndex += 1;
      batches[batchIndex] = firestore.batch();
    }
  });

  for (const batch of batches) {
    await batch.commit();
  }
};
