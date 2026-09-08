import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/20/solid';
import { flexRender } from '@tanstack/react-table';
import { getCoreRowModel, getSortedRowModel, useLegacyTable } from '@tanstack/react-table/legacy';
import { clsx } from 'clsx';
import PropTypes from 'prop-types';
import { forwardRef, useRef, useState } from 'react';
import { twJoin, twMerge } from 'tailwind-merge';

// note: I tried v3 beta of react-virtual but it didn't quite work
const Table = forwardRef(function Table({ columns, data, className, caption, ...props }, forwardedRef) {
  const [sorting, setSorting] = useState(props?.initialState?.sorting ?? []);
  const [columnVisibility] = useState(props?.visibility ?? {});

  const { getHeaderGroups, getRowModel } = useLegacyTable({
    columns,
    data,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    state: {
      sorting,
      columnVisibility,
    },
    onSortingChange: setSorting,
    ...props,
  });

  const parentRef = useRef();

  return (
    <div ref={forwardedRef} className={twMerge('relative flex flex-col', className)}>
      {/* eslint-disable-next-line jsx-a11y/no-noninteractive-tabindex */}
      <div className="h-full overflow-y-auto" ref={parentRef} tabIndex={0}>
        <table className="w-full table-fixed border-collapse">
          <caption className="sr-only">{caption}</caption>
          <thead className="bg-primary-100 text-primary-800 dark:text-primary-300 sticky top-0 text-base dark:bg-slate-950/70">
            {getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <th
                    key={header.id}
                    className={clsx('relative p-2 text-left', {
                      'w-48': header.id === 'key',
                      'w-12 sm:w-24 md:w-40': header.id === 'created',
                      'w-auto': header.id === 'notes',
                      'w-24': header.id === 'action',
                    })}
                  >
                    {header.isPlaceholder ? null : (
                      // eslint-disable-next-line jsx-a11y/click-events-have-key-events, jsx-a11y/no-static-element-interactions
                      <div
                        className={twJoin(
                          header.column.getCanSort() && 'flex cursor-pointer items-center justify-between select-none',
                          header.column.getIsSorted() &&
                            'before:bg-secondary-500 before:absolute before:-bottom-1 before:left-0 before:z-10 before:block before:h-2 before:w-full before:rounded-full',
                        )}
                        onClick={header.column.getToggleSortingHandler()}
                      >
                        {flexRender(header.column.columnDef.header, header.getContext())}
                        {{
                          asc: <ChevronUpIcon className="h-4" />,
                          desc: <ChevronDownIcon className="h-4" />,
                        }[header.column.getIsSorted()] ?? null}
                      </div>
                    )}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody>
            {getRowModel().rows.map((row, index) => {
              const even = index % 2 === 0;
              const odd = !even;

              return (
                <tr
                  key={row.id}
                  className={clsx('border-y-primary-200 dark:border-y-primary-700 border-y', {
                    'text-primary-900 dark:text-primary-300 bg-slate-50 dark:bg-slate-800': even,
                    'bg-primary-100 text-primary-800 dark:text-primary-300 dark:bg-slate-700': odd,
                  })}
                >
                  {row.getVisibleCells().map((cell) => (
                    <td key={cell.id} className="truncate p-2">
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
});

export default Table;

Table.propTypes = {
  caption: PropTypes.string.isRequired,
  className: PropTypes.string,
  /**
   * Corresponds to the same prop in react table (https://tanstack.com/table/v8/docs/api/core/table#columns)
   */
  columns: PropTypes.array.isRequired,
  /**
   * Corresponds to the same prop in react table (https://tanstack.com/table/v8/docs/api/core/table#data)
   */
  data: PropTypes.array.isRequired,
  /**
   * All other props are passed to the useReactTable hook
   */
  visibility: PropTypes.object,
  initialState: PropTypes.shape({
    sorting: PropTypes.array,
  }),
};
