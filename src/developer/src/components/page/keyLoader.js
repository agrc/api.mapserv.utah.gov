import { httpsCallable } from 'firebase/functions';

export const keyLoader = (queryClient, functions, user) => async () => {
  const query = getKeys(functions, user);

  return await queryClient.ensureQueryData(query);
};

export function getKeys(functions, user) {
  return {
    queryKey: ['my keys', user.uid],
    queryFn: () => httpsCallable(functions, 'keys'),
    enabled: user.uid ?? 0 > 0 ? true : false,
    onError: () => 'We had some trouble finding your contacts.',
  };
}
