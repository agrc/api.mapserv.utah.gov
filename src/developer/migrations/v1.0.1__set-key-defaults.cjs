// eslint-disable-next-line no-undef
module.exports.migrate = async ({ firestore }) => {
  const clientKeysSnapshot = await firestore.collection('clients').get();

  for (const doc of clientKeysSnapshot.docs) {
    const collection = await firestore.collection(`clients/${doc.id}/keys`).get();

    for (const doc of collection.docs) {
      const key = doc.data();

      await doc.ref.update({
        deleted: key.deleted === null || key.deleted === undefined ? false : key.deleted,
        disabled: key.disabled === null || key.deleted === undefined ? false : key.disabled,
        lastUsed: 'never',
        usage: 'none',
      });
    }
  }

  const keysSnapshot = await firestore.collectionGroup('keys').get();
  for (const doc of keysSnapshot.docs) {
    const key = doc.data();

    await doc.ref.update({
      deleted: key.deleted === null || key.deleted === undefined ? false : key.deleted,
      disabled: key.disabled === null || key.disabled === undefined ? false : key.disabled,
      lastUsed: 'never',
      usage: 'none',
    });
  }
};
