import { Injectable, signal } from '@angular/core';

export type ToastKind = 'success' | 'error';

export interface Toast {
  readonly kind: ToastKind;
  readonly message: string;
}

const DURATION_MS: Record<ToastKind, number> = {
  success: 2800,
  error: 6000,
};

@Injectable({ providedIn: 'root' })
export class Toaster {
  private readonly state = signal<Toast | null>(null);
  private timer: ReturnType<typeof setTimeout> | null = null;

  readonly current = this.state.asReadonly();

  success(message: string): void {
    this.show({ kind: 'success', message });
  }

  error(message: string): void {
    this.show({ kind: 'error', message });
  }

  dismiss(): void {
    this.clearTimer();
    this.state.set(null);
  }

  private show(toast: Toast): void {
    this.clearTimer();
    this.state.set(toast);
    this.timer = setTimeout(() => {
      this.timer = null;
      this.state.set(null);
    }, DURATION_MS[toast.kind]);
  }

  private clearTimer(): void {
    if (this.timer !== null) {
      clearTimeout(this.timer);
      this.timer = null;
    }
  }
}
