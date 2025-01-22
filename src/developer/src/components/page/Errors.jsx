import { LockClosedIcon } from '@heroicons/react/24/solid';
import PropTypes from 'prop-types';
import { useEffect, useState } from 'react';
import { Link, useNavigate, useRouteError } from 'react-router';

export const RouterErrorPage = ({ error }) => {
  const routeError = useRouteError();

  // authorization error
  if (routeError?.name === 'HTTPError' && routeError?.response.status === 401) {
    return <UnauthorizedRoute routeError={routeError} />;
  }

  return <UnhandledException error={error} routeError={routeError} />;
};
RouterErrorPage.propTypes = {
  error: PropTypes.object,
};

const UnauthorizedRoute = ({ routeError }) => {
  const [rules, setRules] = useState([]);

  useEffect(() => {
    if (!routeError.response.bodyUsed) {
      routeError.response.json().then((data) => {
        setRules(data.errors);
      });
    }
  }, [routeError]);

  return (
    <section>
      <div className="flex flex-col items-center py-6 text-5xl font-black text-gray-800">
        <h1 className="mb-6 block">You are not allowed to be here...</h1>
        <LockClosedIcon className="block h-24 w-24" />
      </div>

      <div className="mx-auto max-w-prose">
        <p>
          This page is not accessible to you for your UIC inventory submission process. If normal usage has brought you
          here, please click the{' '}
          <Link data-style="link" to="contact">
            Contact us
          </Link>{' '}
          link and let us know what happened. Otherwise, please go back to the main page and navigate to your item of
          interest.
        </p>
        <div className="my-4 rounded border border-red-300 bg-red-100/30 px-4">
          <h3 className="my-1 text-lg font-medium text-gray-700">Violated rules</h3>
          <ul className="mb-3">
            {rules.map((rule) => (
              <li className="list-inside list-decimal" key={rule.code}>
                {rule.message}
              </li>
            ))}
          </ul>
        </div>
        <p className="mt-4 text-center text-lg"></p>
        <p className="mt-4">Thank you,</p>
        <p className="mt-1">The UIC Staff</p>
      </div>
    </section>
  );
};
UnauthorizedRoute.propTypes = {
  routeError: PropTypes.object,
};

const UnhandledException = ({ error, routeError }) => {
  const navigate = useNavigate();

  return (
    <section>
      <h2 className="mb-1 text-2xl font-medium text-gray-700">This is a little embarrassing...</h2>
      <p>
        We are really sorry. There was an error in the application that caused it to crash. You may now{' '}
        <button data-style="link" onClick={() => navigate(-1)}>
          go back
        </button>{' '}
        to the previous page and try again or contact us to share the technical details with us from below.
      </p>
      <details className="mt-6">
        <summary>
          <span role="img" aria-label="nerd emoji">
            🤓
          </span>{' '}
          Technical Details
        </summary>
        {routeError && (
          <>
            <label htmlFor="route">Routing Error:</label>
            <pre id="route" className="overflow-auto text-base text-gray-400">{`${routeError}`}</pre>
          </>
        )}
        {error && error.message ? (
          <>
            <label htmlFor="message">Error message:</label>
            <pre id="message" className="overflow-auto text-base text-gray-400">{`${error.message}`}</pre>
          </>
        ) : null}
        {error && error.stack ? (
          <>
            <label htmlFor="stack">Stack trace:</label>
            <pre id="stack" className="overflow-auto text-base text-gray-400">{`${error.stack}`}</pre>
          </>
        ) : null}
        {error && !error.message && !error.stack ? (
          <pre className="overflow-auto text-base text-gray-400">{error.toString()}</pre>
        ) : null}
      </details>
    </section>
  );
};
UnhandledException.propTypes = {
  error: PropTypes.object,
  routeError: PropTypes.object,
};
