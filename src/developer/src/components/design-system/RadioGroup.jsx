import * as Label from '@radix-ui/react-label';
import * as RadixRadioGroup from '@radix-ui/react-radio-group';
import PropTypes from 'prop-types';
import { forwardRef } from 'react';
import { twJoin, twMerge } from 'tailwind-merge';

const RadioGroup = forwardRef(
  (
    {
      ariaLabel,
      className,
      defaultValue,
      id,
      inline,
      items,
      label,
      name,
      onChange,
      required,
      value,
    },
    ref,
  ) => {
    return (
      <div
        className={twMerge(
          inline ? 'inline-flex items-center' : 'flex flex-col',
          className,
        )}
      >
        <Label.Root asChild className="mr-2" htmlFor={id}>
          <strong>
            {label}
            {required && (
              <span className="ml-[0.1rem] text-fuchsia-500">*</span>
            )}
          </strong>
        </Label.Root>
        <RadixRadioGroup.Root
          ref={ref}
          name={name}
          aria-label={ariaLabel}
          className="flex flex-col items-start"
          defaultValue={defaultValue}
          value={value}
          onValueChange={onChange}
        >
          {items.map((item) => {
            const id = `${ariaLabel}-${item.value}`;

            return (
              <div className="group flex items-center" key={id}>
                <div
                  className={twJoin(
                    'rounded-full p-1',
                    !item.disabled &&
                      'group-hover:bg-mustard-200 group-focus:bg-mustard-200 group-active:bg-mustard-200',
                  )}
                >
                  <RadixRadioGroup.Item
                    className={twJoin(
                      'flex h-4 w-4 items-center justify-center rounded-full border ',
                      item.disabled
                        ? 'border-slate-300 bg-slate-50 data-[state=checked]:bg-slate-300 data-[state=checked]:text-white'
                        : 'border-slate-500 bg-white',
                    )}
                    value={item.value}
                    id={id}
                    disabled={item.disabled}
                  >
                    <RadixRadioGroup.Indicator
                      className={twJoin(
                        'after:block after:h-4 after:w-4',
                        'after:rounded-full after:content-[""]',
                        'after:border-4 after:border-mustard-500',
                      )}
                    />
                  </RadixRadioGroup.Item>
                </div>
                <label
                  htmlFor={id}
                  className={twJoin(
                    'pl-1 leading-5 ',
                    item.disabled
                      ? 'cursor-not-allowed text-slate-300'
                      : 'cursor-pointer',
                  )}
                >
                  {item.label}
                </label>
              </div>
            );
          })}
        </RadixRadioGroup.Root>
      </div>
    );
  },
);
RadioGroup.displayName = 'RadioGroup';
RadioGroup.propTypes = {
  ariaLabel: PropTypes.string.isRequired,
  className: PropTypes.string,
  disabled: PropTypes.bool,
  defaultValue: PropTypes.string,
  id: PropTypes.string,
  inline: PropTypes.bool,
  items: PropTypes.arrayOf(
    PropTypes.shape({
      value: PropTypes.string.isRequired,
      label: PropTypes.string.isRequired,
    }),
  ).isRequired,
  label: PropTypes.string.isRequired,
  name: PropTypes.string,
  onChange: PropTypes.func,
  required: PropTypes.bool,
  value: PropTypes.string,
};

export default RadioGroup;
