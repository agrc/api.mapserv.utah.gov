export function Component() {
  return (
    <section className="mx-auto max-w-full px-3 lg:px-0">
      <div className="flex flex-wrap">
        <div className="lg:flex-basis-auto min-h-[480px] w-full max-w-full py-10 lg:min-h-[980px] lg:w-1/2 lg:shrink-0 lg:grow-0 lg:py-[114px]">
          <div className="mx-auto grid w-full max-w-[480px] gap-6">
            <h2 className="text-wavy-800 dark:text-slate-100">Manage keys</h2>
          </div>
        </div>
      </div>
    </section>
  );
}
Component.displayName = 'ManageKey';
