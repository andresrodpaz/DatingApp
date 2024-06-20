import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'customDateParser' })
export class CustomDateParser implements PipeTransform {
  transform(value: Date | string): string {
    if (!value) return '';

    const currentDate = new Date();
    const date = new Date(value);

    // Convertir ambas fechas a UTC
    const utcCurrentDate = new Date(Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()));
    const utcDate = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));

    // Si las fechas son iguales, devuelve 'today'
    if (utcCurrentDate.getTime() === utcDate.getTime()) {
      return 'today';
    }

    const diffTime = Math.abs(utcCurrentDate.getTime() - utcDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 1) {
      return 'yesterday';
    } else if (diffDays < 7) {
      return `${diffDays} days ago`;
    } else if (diffDays < 14) {
      return 'a week ago';
    } else if (diffDays < 30) {
      return '2 weeks ago';
    } else if (diffDays < 60) {
      return 'a month ago';
    } else if (diffDays < 365) {
      return 'a few months ago';
    } else {
      const diffYears = Math.floor(diffDays / 365);
      return `${diffYears} years ago`;
    }
  }
}
