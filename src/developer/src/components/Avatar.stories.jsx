import Avatar from './Avatar';

export default {
  title: 'Self service/Avatar',
  component: Avatar,
};

export const Default = () => {
  return (
    <section className="grid gap-6">
      <h4>Anonymous gets no avatar</h4>
      <Avatar />
      <Avatar anonymous={true} />
      <Avatar anonymous={null} />
      <h4>No gravatar gets initials</h4>
      <Avatar
        anonymous={false}
        user={{ email: 'test@test.com', displayName: 'Test User' }}
      />
      <Avatar
        anonymous={false}
        user={{ email: 'test@test.com', displayName: 'Test My User' }}
      />
      <Avatar
        anonymous={false}
        user={{ email: 'test@test.com', displayName: 'Test My Long User' }}
      />
      <h4>Gravatar shows image</h4>
      <Avatar
        anonymous={false}
        user={{ email: 'sgourley@utah.gov', displayName: 'Test User' }}
      />
    </section>
  );
};
