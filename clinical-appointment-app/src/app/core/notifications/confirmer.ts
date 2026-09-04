import { Injectable, computed, signal } from '@angular/core';

export interface ConfirmRequest {
  readonly title: string;
  readonly body: string;
  readonly confirmLabel?: string;
  readonly cancelLabel?: string;
}

export interface ConfirmPrompt {
  readonly title: string;
  readonly body: string;
  readonly confirmLabel: string | null;
  readonly cancelLabel: string;
}

interface Pending {
  readonly prompt: ConfirmPrompt;
  readonly resolve: (confirmed: boolean) => void;
}

@Injectable({ providedIn: 'root' })
export class Confirmer {
  private readonly pending = signal<Pending | null>(null);

  readonly current = computed<ConfirmPrompt | null>(() => this.pending()?.prompt ?? null);

  ask(request: ConfirmRequest): Promise<boolean> {
    this.settle(false);

    return new Promise<boolean>((resolve) => {
      this.pending.set({
        prompt: {
          title: request.title,
          body: request.body,
          confirmLabel: request.confirmLabel ?? null,
          cancelLabel: request.cancelLabel ?? 'Cancel',
        },
        resolve,
      });
    });
  }

  confirm(): void {
    this.settle(true);
  }

  dismiss(): void {
    this.settle(false);
  }

  private settle(confirmed: boolean): void {
    const pending = this.pending();

    if (pending === null) {
      return;
    }

    this.pending.set(null);
    pending.resolve(confirmed);
  }
}
