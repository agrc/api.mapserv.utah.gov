import PropTypes from 'prop-types';

const Footer = ({ className }) => {
  return (
    <footer className={className}>
      <div className="relative flex flex-col flex-wrap items-center justify-between border-t border-dashed border-t-secondary-300 p-8 lg:flex-row">
        <div className="flex flex-col items-center gap-4 text-center lg:h-8 lg:flex-row lg:divide-x lg:text-left">
          <div className="h-full" id="utah-logo-svg">
            <svg
              className="block h-8 w-auto fill-white"
              viewBox="0 0 107 30.51"
              role="img"
            >
              <g>
                <path d="m12.44,30.51c-4.21,0-7.33-1.22-9.38-3.66C1.02,24.4,0,20.61,0,15.48V0h7.93v16.4c0,2.67.36,4.55,1.08,5.65.77,1.12,2.08,1.74,3.43,1.64,1.36.1,2.68-.52,3.48-1.63.75-1.09,1.13-2.97,1.13-5.65V0h7.65v15.48c0,5.13-1,8.92-3,11.36-2,2.44-5.09,3.66-9.26,3.66Zm24.42-.56V6.64h-7.93V0h23.78v6.64h-7.93v23.31h-7.92Zm26.17-14.56l-.51,2.07h5.53l-.51-2.07c-.37-1.44-.74-3.01-1.11-4.7-.37-1.69-.74-3.29-1.11-4.79h-.18c-.34,1.53-.68,3.14-1.04,4.82-.35,1.68-.71,3.24-1.08,4.68Zm-11.52,14.56L60.64,0h9.58l9.12,29.95h-8.39l-1.48-6.36h-8.38l-1.47,6.36h-8.11Zm30.69,0V0h7.93v11.15h8.94V0h7.93v29.95h-7.93v-11.89h-8.94v11.89h-7.93Z" />
              </g>
            </svg>
          </div>
          <div className="pl-4">
            <div className="text-lg font-semibold text-white">
              An official website of the{' '}
              <span className="whitespace-no-wrap">State of Utah</span>
            </div>
            <div className="text-sm text-white">©2023 State of Utah</div>
          </div>
        </div>
        <div className="pt-6 text-center text-sm text-white lg:pt-0">
          <div className="space-x-2 whitespace-pre-wrap lg:space-x-4">
            <a href="https://www.utah.gov" target="__blank" rel="noreferrer">
              Utah.gov Home
            </a>
            <span className="border-l-2" aria-hidden></span>
            <a
              href="https://www.utah.gov/disclaimer.html"
              target="__blank"
              rel="noreferrer"
            >
              Terms of use
            </a>
            <span className="border-l-2" aria-hidden></span>
            <a
              href="https://www.utah.gov/privacypolicy.html"
              target="__blank"
              rel="noreferrer"
            >
              Privacy policy
            </a>
            <span className="border-l-2" aria-hidden></span>
            <a
              href="https://www.utah.gov/accessibility.html"
              target="__blank"
              rel="noreferrer"
            >
              Accessibility
            </a>
            <span className="border-l-2" aria-hidden></span>
            <a
              href="https://www.utah.gov/translate.html"
              target="__blank"
              rel="noreferrer"
            >
              Translate
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
};

Footer.propTypes = {
  className: PropTypes.string,
};

export default Footer;
