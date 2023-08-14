module.exports.migrate = async ({ firestore }) => {
  const generateRegexFromPattern = (
    await import('../functions/https/createKey.js')
  ).generateRegexFromPattern;
  const clientKeysSnapshot = await firestore.collection('clients').get();

  await clientKeysSnapshot.forEach(async (doc) => {
    const collection = await firestore
      .collection(`clients/${doc.id}/keys`)
      .get();

    collection.forEach(async (doc) => {
      const key = doc.data();
      if (key.type !== 'browser') {
        return;
      }

      await doc.ref.update({
        regularExpression: generateRegexFromPattern(key.pattern),
      });
    });
  });

  const keysSnapshot = await firestore.collectionGroup('keys').get();
  await keysSnapshot.forEach(async (doc) => {
    const key = doc.data();

    if (key.type !== 'browser') {
      return;
    }

    await doc.ref.update({
      regularExpression: generateRegexFromPattern(key.pattern),
    });
  });
};
