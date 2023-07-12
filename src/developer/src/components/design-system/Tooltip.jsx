import * as RadixTooltip from '@radix-ui/react-tooltip';
import PropTypes from 'prop-types';
import { isValidElement } from 'react';

export default function Tooltip({ open, trigger, children, delayDuration }) {
  return (
    <RadixTooltip.Provider>
      <RadixTooltip.Root open={open} delayDuration={delayDuration}>
        {/*
          Mark asChild true if the trigger is a component as opposed to a string.
          This prevents the console error about rendering a button within a button if the component
          contains a button.
        */}
        <RadixTooltip.Trigger asChild={isValidElement(trigger)}>
          {trigger}
        </RadixTooltip.Trigger>
        <RadixTooltip.Portal>
          <RadixTooltip.Content
            side="bottom"
            className="z-10 mr-2 rounded-md bg-white px-2 py-1 text-sm text-slate-500 shadow-md outline-none data-[state=delayed-open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=delayed-open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=delayed-open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 dark:bg-slate-900 dark:text-slate-300"
            sideOffset={4}
          >
            {children}
            <RadixTooltip.Arrow className="fill-white dark:fill-slate-900" />
          </RadixTooltip.Content>
        </RadixTooltip.Portal>
      </RadixTooltip.Root>
    </RadixTooltip.Provider>
  );
}

Tooltip.propTypes = {
  open: PropTypes.bool,
  trigger: PropTypes.node.isRequired,
  /**
   * The content of the tooltip
   */
  children: PropTypes.node.isRequired,
  delayDuration: PropTypes.number,
};
