module.exports.migrate = async ({ firestore, FieldValue }) => {
  const querySnapshot = await firestore.collection('clients').get();

  for (const doc of querySnapshot.docs) {
    const client = doc.data();
    const collection = firestore.collection(`clients/${doc.id}/keys`);

    for (const key of client.keys) {
      await collection.doc(key.id).set(key);
    }

    await doc.ref.update({
      keys: FieldValue.delete(),
    });
  }
};
