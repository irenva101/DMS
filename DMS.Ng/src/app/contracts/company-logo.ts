const PALETTE_SIZE = 6;

const SUFFIXES = new Set(['d.o.o.', 'd.o.o', 'a.d.', 'ad']);

export function companyInitials(name: string): string {
  const words = name.split(' ').filter((w) => !SUFFIXES.has(w.toLowerCase()));
  return words.slice(0, 2).map((w) => w[0]).join('').toUpperCase();
}

export function companyColor(id: string): string {
  let hash = 0;
  for (let i = 0; i < id.length; i++) hash = (hash * 31 + id.charCodeAt(i)) >>> 0;
  return `var(--company-color-${(hash % PALETTE_SIZE) + 1})`;
}
