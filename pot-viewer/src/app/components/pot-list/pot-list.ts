import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PotService } from '../../services/pot.service';
import { PotInstance, MonetaryValue } from '../../models/pot-instance.model';
import { HttpClientModule } from '@angular/common/http';
import { ConversionRate, CurrencyTotalOverview } from '../../models/pot-response.model';

@Component({
  selector: 'app-pot-list',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './pot-list.html',
  styleUrl: './pot-list.scss'
})
export class PotList implements OnInit {
  potInstances = signal<PotInstance[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  totalValue = signal<MonetaryValue | null>(null);
  conversionRates = signal<ConversionRate[]>([]);
  currencyOverviews = signal<CurrencyTotalOverview[]>([]);
  responseDate = signal<Date | null>(null);

  constructor(private potService: PotService) {
  }

  ngOnInit(): void {
    this.loadPots();
  }

  loadPots(): void {
    this.potInstances.set([]);
    this.loading.set(true);
    this.error.set(null);

    this.potService.getPots().subscribe({
      next: (response) => {
        this.potInstances.set(response.potInstances || []);
        this.totalValue.set(response.total);
        this.conversionRates.set(response.conversionRates || []);
        this.currencyOverviews.set(response.currencyTotalOverviews || []);
        this.responseDate.set(response.date);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading pots:', error);
        this.error.set('An error occurred while loading pot accounts.');
        this.loading.set(false);
      }
    });
  }

  getActivePotCount(): number {
    return this.potInstances()?.filter(pot => pot.isActive).length || 0;
  }
}
