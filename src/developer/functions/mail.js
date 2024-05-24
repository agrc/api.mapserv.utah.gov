import client from '@sendgrid/client';

export const mailingListSignUp = async (data, apiKey) => {
  client.setApiKey(apiKey);

  let response;
  const [first_name, last_name] = data.displayName.split(' ');

  const body = {
    list_ids: ['272a49d6-7d98-46bd-9312-7310e8e73ba9'],
    contacts: [
      {
        email: data.email,
        first_name,
        last_name,
      },
    ],
  };

  try {
    response = await client.request({
      method: 'PUT',
      url: '/v3/marketing/contacts',
      body,
    });

    if (response.statusCode !== 202) {
      error('[mail::mailingListSignUp] sendgrid job error', {
        response: response.body,
        data,
      });

      return false;
    }
  } catch (errorMessage) {
    error('[mail::mailingListSignUp] sendgrid error', {
      errorMessage,
      data,
    });

    return false;
  }

  return true;
};
