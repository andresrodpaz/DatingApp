import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  ngOnInit(): void {
    const stackContainer = document.querySelector('.stack-container') as HTMLElement;
    const cardNodes = document.querySelectorAll('.card-container') as NodeListOf<HTMLElement>;
    const consoleNodes = document.querySelectorAll('.writing') as NodeListOf<HTMLElement>;
    const perspecNodes = document.querySelectorAll('.perspec') as NodeListOf<HTMLElement>;
    const perspec = document.querySelector('.perspec') as HTMLElement;
    const card = document.querySelector('.card') as HTMLElement;

    let counter = stackContainer.children.length;

    // Function to generate random number
    function randomIntFromInterval(min: number, max: number): number {
      return Math.floor(Math.random() * (max - min + 1) + min);
    }

    if (card) {
      // After tilt animation, fire the explode animation
      card.addEventListener('animationend', () => {
        perspecNodes.forEach(elem => {
          elem.classList.add('explode');
        });
      });
    }

    if (perspec) {
      // After explode animation, do a bunch of stuff
      perspec.addEventListener('animationend', (e: AnimationEvent) => {
        if (e.animationName === 'explode') {
          cardNodes.forEach(elem => {
            // Add hover animation class
            elem.classList.add('pokeup');

            // Add event listener to throw card on click
            elem.addEventListener('click', () => {
              const updown = [800, -800];
              const randomY = updown[Math.floor(Math.random() * updown.length)];
              const randomX = Math.floor(Math.random() * 1000) - 1000;
              elem.style.transform = `translate(${randomX}px, ${randomY}px) rotate(-540deg)`;
              elem.style.transition = 'transform 1s ease, opacity 2s';
              elem.style.opacity = '0';
              counter--;
              if (counter === 0) {
                stackContainer.style.width = '0';
                stackContainer.style.height = '0';
              }
            });
          });
        }
      });
    }
  }
}
