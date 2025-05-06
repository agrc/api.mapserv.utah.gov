// eslint-disable-next-line no-undef
module.exports.migrate = async ({ firestore, FieldValue }) => {
  const keysSnap = await firestore.collectionGroup('keys').get();

  const batches = [firestore.batch()];
  let size = 0;
  let batchIndex = 0;

  for (const doc of keysSnap.docs) {
    size += 1;
    const data = doc.data();

    batches[batchIndex].set(
      doc.ref,
      {
        flags: {
          deleted: data.deleted,
          disabled: data.disabled ?? false,
          server: data.type === 'server',
          production: data.mode === 'production',
        },
      },
      { merge: true },
    );

    batches[batchIndex].update(doc.ref, {
      status: FieldValue.delete(),
      deleted: FieldValue.delete(),
      disabled: FieldValue.delete(),
      mode: FieldValue.delete(),
      type: FieldValue.delete(),
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
