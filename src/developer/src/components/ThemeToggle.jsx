import { MoonIcon, SunIcon } from '@heroicons/react/20/solid';
import clsx from 'clsx';
import { useEffect, useState } from 'react';
const themes = ['light', 'dark'];

const icons = [
  <SunIcon className="h-5 fill-current" key="light" />,
  <MoonIcon className="h-5 fill-current" key="dark" />,
];

const ThemeToggle = () => {
  const [theme, setTheme] = useState(() => {
    if (typeof localStorage !== 'undefined' && localStorage.getItem('theme')) {
      return localStorage.getItem('theme');
    }
    if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }
    return 'light';
  });

  useEffect(() => {
    const root = document.documentElement;
    if (theme === 'light') {
      root.classList.remove('dark');
    } else {
      root.classList.add('dark');
    }
  }, [theme]);

  return (
    <div className="inline-flex items-center gap-2 rounded-full bg-wavy-500 px-3 py-2">
      {themes.map((t, i) => {
        const icon = icons[i];
        const checked = t === theme;
        return (
          <label
            key={i}
            className={clsx(
              'relative flex cursor-pointer items-center justify-center opacity-60',
              {
                'text-white': checked,
              },
            )}
          >
            {icon}
            <input
              className="absolute inset-0 -z-10 opacity-0"
              type="radio"
              name="theme-toggle"
              checked={checked}
              value={t}
              title={`Use ${t} theme`}
              aria-label={`Use ${t} theme`}
              onChange={() => {
                localStorage.setItem('theme', t);
                setTheme(t);
              }}
            />
          </label>
        );
      })}
    </div>
  );
};

export default ThemeToggle;
