export class RequestGate {
  private token = 0;

  next(): number {
    return ++this.token;
  }

  invalidate(): void {
    ++this.token;
  }

  isCurrent(candidate: number): boolean {
    return candidate === this.token;
  }
}
