import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PotService } from '../../services/pot.service';
import { Pot } from '../../models/pot.model';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-pot-list',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './pot-list.html',
  styleUrl: './pot-list.scss'
})
export class PotList implements OnInit {
  pots = signal<Pot[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  constructor(private potService: PotService) {
  }

  ngOnInit(): void {
    this.loadPots();
  }

  loadPots(): void {
    this.pots.set([]);
    this.loading.set(true);
    this.error.set(null);

    setTimeout(() => {
      try {
        const potsData = this.retrievePots();
        this.pots.set(potsData);
      }
      catch (error) {
        console.error('Error loading pots:', error);
        this.error.set('An error occurred while loading pots.');
      }
      finally {
        this.loading.set(false);
      }
    }, 3000);
  }

  private retrievePots(): Pot[] {
    return [
      {
        id: 1,
        name: 'Golden Vase',
        description: 'A shimmering golden vase with intricate engravings',
        type: 'Vase',
        material: 'Gold',
        color: 'Gold'
      },
      {
        id: 2,
        name: 'Ceramic Blue Bowl',
        description: 'A delicate blue ceramic bowl',
        type: 'Bowl',
        material: 'Ceramic',
        color: 'Blue'
      },
      {
        id: 3,
        name: 'Ancient Clay Pot',
        description: 'An ancient pot with historical markings',
        type: 'Pot',
        material: 'Clay',
        color: 'Brown'
      },
      {
        id: 4,
        name: 'Ancient Clay Pot',
        description: 'An ancient pot with historical markings',
        type: 'Pot',
        material: 'Clay',
        color: 'Brown'
      }
    ];
  }
}
