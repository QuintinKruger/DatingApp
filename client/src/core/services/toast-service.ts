import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  // in course we avoid using 3rd party services and because of this we 
  // will be writing alot of javascript code here - tailwind and daisy UI 
  // is used to only style components, it contains no JS to perform any actions 
  // we might want to - this will be up to us do or up to a 3rd party component 
  constructor() {
    this.createToastContainer();
  }

  private createToastContainer() {
    // recall services are singleton so they exist for entire app 
    // lifecycle. Because of this we can reuse some things hence why 
    // we check if something eixsts first before creating it 
    if (!document.getElementById('toast-container')) {
      const container = document.createElement('div')
      container.id = 'toast-container'
      container.className = 'toast toast-bottom toast-end z-50' // tailwind / daisy UI classes 
      document.body.appendChild(container);
    }
  }

  private createToastElement(message: string, alertClass: string, duration = 5000) {
    const toastContainer = document.getElementById("toast-container");
    if (!toastContainer) return

    const toast = document.createElement('div');
    toast.classList.add('alert', alertClass, 'shadow-lg');
    // ``allows for template string 
    // define the html content that will be inside the toast div 
    toast.innerHTML = `
      <span>${message}</span>
      <button class="ml-4 btn btn-sm btn-ghost">x</button>
    `

    // add button click functionality 
    toast.querySelector('button')?.addEventListener('click', () => {
      toastContainer.removeChild(toast)
    })

    toastContainer.append(toast);

    setTimeout(() => {
      if (toastContainer.contains(toast)) {
        toastContainer.removeChild(toast)
      }

    }, duration)
  }

  success(message: string, duration?: number) {
    this.createToastElement(message, 'alert-success', duration)
  };
  error(message: string, duration?: number) {
    this.createToastElement(message, 'alert-error', duration)
  };

  warning(message: string, duration?: number) {
    this.createToastElement(message, 'alert-warning', duration)
  };

  info(message: string, duration?: number) {
    this.createToastElement(message, 'alert-info', duration)
  };

}
