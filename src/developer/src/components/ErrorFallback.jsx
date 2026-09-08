import { Button } from '@ugrc/utah-design-system/components/Button';
import PropTypes from 'prop-types';

export const MainErrorFallback = ({ error, resetErrorBoundary }) => {
  return (
    <div className="static flex h-screen w-screen items-center justify-center">
      <div className="flex-col items-center">
        <h1>Something went wrong</h1>
        <pre className="text-red-500">{error.message}</pre>
        <Button className="w-full" onPress={resetErrorBoundary}>
          Try again
        </Button>
      </div>
    </div>
  );
};
MainErrorFallback.propTypes = {
  error: PropTypes.object,
  resetErrorBoundary: PropTypes.func,
};
