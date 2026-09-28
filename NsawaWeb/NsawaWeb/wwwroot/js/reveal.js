// Scroll reveal for the landing page: elements with [data-reveal] fade and rise into view once.
// Content stays visible without JS, and motion is skipped for people who prefer reduced motion.
export function init(root) {
    if (!root || !("IntersectionObserver" in window)) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    const items = root.querySelectorAll("[data-reveal]");
    const viewportBottom = window.innerHeight;

    // Anything already on screen shows immediately; only content below the fold animates.
    items.forEach(el => {
        if (el.getBoundingClientRect().top < viewportBottom * 0.9) el.classList.add("is-visible");
    });
    root.classList.add("reveal-ready");

    const observer = new IntersectionObserver(entries => {
        for (const entry of entries) {
            if (!entry.isIntersecting) continue;
            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
        }
    }, { rootMargin: "0px 0px -8% 0px", threshold: 0.12 });

    items.forEach(el => { if (!el.classList.contains("is-visible")) observer.observe(el); });
    root._revealObserver = observer;
}

export function dispose(root) {
    root?._revealObserver?.disconnect();
}
