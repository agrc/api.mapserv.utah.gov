module.exports.migrate = async ({ firestore, FieldValue }) => {
  const collectionSnapshot = await firestore.collectionGroup('keys').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  for (const doc of collectionSnapshot.docs) {
    size += 1;

    batches[batchIndex].update(doc.ref, {
      usage: FieldValue.delete(),
      lastUsed: FieldValue.delete(),
    });

    if (size % 400 === 0) {
      batchIndex += 1;
      batches[batchIndex] = firestore.batch();
    }
  }

  for (const batch of batches) {
    await batch.commit();
  }
};
