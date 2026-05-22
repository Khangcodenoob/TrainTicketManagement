namespace TrainTicketWinForms;

/// <summary>
/// Áp dụng icon emoji vào header của tất cả DataGridView sau khi bind dữ liệu.
/// </summary>
public partial class Form1
{
    // ═══════════════════════════════════════════════════════════
    //  ICON MAPPING — property name → display header với icon
    // ═══════════════════════════════════════════════════════════

    private static readonly Dictionary<string, string> ColumnIcons = new()
    {
        // ── RouteItem ──────────────────────────────────────────
        { "RouteId",           "🆔  Mã tuyến"         },
        { "DepartureStation",  "📍  Ga đi"             },
        { "ArrivalStation",    "📍  Ga đến"            },
        { "DistanceKm",        "📏  Khoảng cách (km)"  },
        { "Status",            "📊  Trạng thái"        },

        // ── TrainTripItem ──────────────────────────────────────
        { "TrainCode",         "🚂  Số tàu"            },
        { "DepartureTime",     "🕐  Giờ đi"            },
        { "ArrivalTime",       "🕑  Giờ đến"           },
        { "TotalSeats",        "💺  Tổng ghế"          },
        { "AvailableSeats",    "✅  Còn ghế"           },
        { "BaseTicketPrice",   "💰  Giá vé cơ bản"     },

        // ── CustomerItem ───────────────────────────────────────
        { "CustomerId",        "🆔  Mã KH"             },
        { "FullName",          "👤  Họ và tên"         },
        { "PhoneNumber",       "📞  Số điện thoại"     },
        { "Email",             "📧  Email"             },
        { "IdentityNumber",    "🪪  Số CCCD"           },
        { "Address",           "📍  Địa chỉ"           },

        // ── TicketItem ─────────────────────────────────────────
        { "TicketId",          "🆔  Mã vé (ID)"        },
        { "TicketCode",        "🎫  Mã code vé"        },
        { "SeatNumber",        "💺  Số ghế"            },
        { "Price",             "💰  Giá vé (VND)"      },
        { "BookingDate",       "📅  Ngày đặt"          },
        { "PaymentStatus",     "💳  Thanh toán"        },
        { "TicketStatus",      "📊  Trạng thái vé"     },
    };

    // ── Apply icons to a grid's column headers ──────────────────
    private static void ApplyColumnIcons(DataGridView grid)
    {
        foreach (DataGridViewColumn col in grid.Columns)
        {
            if (ColumnIcons.TryGetValue(col.Name, out var icon))
                col.HeaderText = icon;
        }
    }

    // ── Hook DataBindingComplete on all auto-bound grids ─────────
    private void RegisterGridIconEvents()
    {
        _routesGrid.DataBindingComplete    += (_, _) => ApplyColumnIcons(_routesGrid);
        _tripsGrid.DataBindingComplete     += (_, _) => ApplyColumnIcons(_tripsGrid);
        _customersGrid.DataBindingComplete += (_, _) => ApplyColumnIcons(_customersGrid);
        _ticketsGrid.DataBindingComplete   += (_, _) => ApplyColumnIcons(_ticketsGrid);
    }
}
