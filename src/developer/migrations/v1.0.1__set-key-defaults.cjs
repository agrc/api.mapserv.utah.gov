module.exports.migrate = async ({ firestore }) => {
  const clientKeysSnapshot = await firestore.collection('clients').get();

  await clientKeysSnapshot.forEach(async (doc) => {
    const collection = await firestore
      .collection(`clients/${doc.id}/keys`)
      .get();

    collection.forEach(async (doc) => {
      const key = doc.data();

      await doc.ref.update({
        deleted:
          key.deleted === null || key.deleted === undefined
            ? false
            : key.deleted,
        disabled:
          key.disabled === null || key.deleted === undefined
            ? false
            : key.disabled,
        lastUsed: 'never',
        usage: 'none',
      });
    });
  });

  const keysSnapshot = await firestore.collectionGroup('keys').get();
  await keysSnapshot.forEach(async (doc) => {
    const key = doc.data();

    await doc.ref.update({
      deleted:
        key.deleted === null || key.deleted === undefined ? false : key.deleted,
      disabled:
        key.disabled === null || key.disabled === undefined
          ? false
          : key.disabled,
      lastUsed: 'never',
      usage: 'none',
    });
  });
};
