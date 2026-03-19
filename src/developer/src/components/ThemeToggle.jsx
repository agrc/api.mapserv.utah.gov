import { MoonIcon, SunIcon } from '@heroicons/react/20/solid';
import { clsx } from 'clsx';
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

  const updateTheme = (nextTheme) => {
    localStorage.setItem('theme', nextTheme);
    setTheme(nextTheme);
  };

  return (
    <div
      className="inline-flex items-center gap-2 rounded-full bg-primary-500 px-3 py-2"
      role="group"
      aria-label="Theme"
    >
      {themes.map((t, i) => {
        const icon = icons[i];
        const checked = t === theme;
        return (
          <button
            key={i}
            type="button"
            className={clsx('relative flex items-center justify-center rounded-full hover:bg-white/30', {
              'text-white hover:text-white/90': checked,
              'dark:text-primary-800': !checked,
            })}
            aria-label={`${t} theme`}
            aria-pressed={checked}
            title={`${t} theme`}
            onClick={() => updateTheme(t)}
          >
            <span aria-hidden="true">{icon}</span>
          </button>
        );
      })}
    </div>
  );
};

export default ThemeToggle;
