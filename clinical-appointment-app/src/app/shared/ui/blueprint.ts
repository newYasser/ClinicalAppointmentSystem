import { Directive, ElementRef, OnInit, Renderer2, inject } from '@angular/core';

const CORNERS = ['tl', 'tr', 'bl', 'br'] as const;

@Directive({
  selector: '[appBlueprint]',
  host: { class: 'blueprint' },
})
export class Blueprint implements OnInit {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly renderer = inject(Renderer2);

  ngOnInit(): void {
    for (const corner of CORNERS) {
      const mark = this.renderer.createElement('i');
      this.renderer.addClass(mark, 'corner');
      this.renderer.addClass(mark, corner);
      this.renderer.appendChild(this.host.nativeElement, mark);
    }
  }
}
