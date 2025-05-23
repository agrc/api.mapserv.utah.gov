// eslint-disable-next-line no-undef
module.exports.migrate = async ({ firestore, FieldValue }) => {
  const clientsSnap = await firestore.collectionGroup('clients').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  for (const doc of clientsSnap.docs) {
    size += 1;

    batches[batchIndex].update(doc.ref, {
      claimedAccounts: FieldValue.delete(),
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
