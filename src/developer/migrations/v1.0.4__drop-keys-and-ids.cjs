module.exports.migrate = async ({ firestore, FieldValue }) => {
  const clientsSnap = await firestore.collectionGroup('clients').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  for (const doc of clientsSnap.docs) {
    size += 1;

    batches[batchIndex].update(doc.ref, {
      keyIds: FieldValue.delete(),
    });

    const keys = await firestore
      .collection(`clients/${doc.id}/keys`)
      .listDocuments();

    for (const keyDoc of keys) {
      size += 1;
      batches[batchIndex].delete(keyDoc);

      if (size % 400 === 0) {
        batchIndex += 1;
        batches[batchIndex] = firestore.batch();
      }
    }

    if (size % 400 === 0) {
      batchIndex += 1;
      batches[batchIndex] = firestore.batch();
    }
  }

  for (const batch of batches) {
    await batch.commit();
  }
};
