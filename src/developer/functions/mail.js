import client from '@sendgrid/client';
import { error } from 'firebase-functions/logger';

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

    // if response is an array grab first element
    if (Array.isArray(response)) {
      response = response[0];
    }

    if (response.statusCode !== 202) {
      error('[mail::mailingListSignUp] sendgrid job error', {
        sendgridResponse: JSON.stringify(response),
        sendgridData: body,
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
