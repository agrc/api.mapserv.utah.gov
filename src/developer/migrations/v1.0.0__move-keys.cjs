module.exports.migrate = async ({ firestore, FieldValue }) => {
  const querySnapshot = await firestore.collection('clients').get();

  await querySnapshot.forEach(async (doc) => {
    const client = doc.data();
    const collection = firestore.collection(`clients/${doc.id}/keys`);

    client.keys.forEach(async (key) => {
      await collection.doc(key.id).set(key);
    });

    await doc.ref.update({
      keys: FieldValue.delete(),
    });
  });
};
